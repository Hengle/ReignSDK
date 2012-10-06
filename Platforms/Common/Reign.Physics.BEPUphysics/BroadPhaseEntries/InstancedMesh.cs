﻿using System;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Materials;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.ResourceManagement;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.OtherSpaceStages;
using Reign.Core;

namespace BEPUphysics.Collidables
{
    ///<summary>
    /// Collidable mesh which can be created from a reusable InstancedMeshShape.
    /// Very little data is needed for each individual InstancedMesh object, allowing
    /// a complicated mesh to be repeated many times.  Since the hierarchy used to accelerate
    /// collisions is purely local, it may be marginally slower than an individual StaticMesh.
    ///</summary>
    public class InstancedMesh : StaticCollidable
    {

        internal AffineTransform3 worldTransform;
        ///<summary>
        /// Gets or sets the world transform of the mesh.
        ///</summary>
        public AffineTransform3 WorldTransform
        {
            get
            {
                return worldTransform;
            }
            set
            {
                worldTransform = value;
                Shape.ComputeBoundingBox(ref value, out boundingBox);
            }
        }

        /// <summary>
        /// Updates the bounding box to the current state of the entry.
        /// </summary>
        public override void UpdateBoundingBox()
        {
            Shape.ComputeBoundingBox(ref worldTransform, out boundingBox);
        }


        ///<summary>
        /// Constructs a new InstancedMesh.
        ///</summary>
        ///<param name="meshShape">Shape to use for the instance.</param>
        public InstancedMesh(InstancedMeshShape meshShape)
            : this(meshShape, AffineTransform3.Identity)
        {
        }

        ///<summary>
        /// Constructs a new InstancedMesh.
        ///</summary>
        ///<param name="meshShape">Shape to use for the instance.</param>
        ///<param name="worldTransform">Transform to use for the instance.</param>
        public InstancedMesh(InstancedMeshShape meshShape, AffineTransform3 worldTransform)
        {
            this.worldTransform = worldTransform;
            base.Shape = meshShape;
            Events = new ContactEventManager<InstancedMesh>();


        }

        ///<summary>
        /// Gets the shape used by the instanced mesh.
        ///</summary>
        public new InstancedMeshShape Shape
        {
            get
            {
                return (InstancedMeshShape)shape;
            }
        }

        internal TriangleSidedness sidedness = TriangleSidedness.DoubleSided;
        ///<summary>
        /// Gets or sets the sidedness of the mesh.  This can be used to ignore collisions and rays coming from a direction relative to the winding of the triangle.
        ///</summary>
        public TriangleSidedness Sidedness
        {
            get
            {
                return sidedness;
            }
            set
            {
                sidedness = value;
            }
        }

        internal bool improveBoundaryBehavior = true;
        /// <summary>
        /// Gets or sets whether or not the collision system should attempt to improve contact behavior at the boundaries between triangles.
        /// This has a slight performance cost, but prevents objects sliding across a triangle boundary from 'bumping,' and otherwise improves
        /// the robustness of contacts at edges and vertices.
        /// </summary>
        public bool ImproveBoundaryBehavior
        {
            get
            {
                return improveBoundaryBehavior;
            }
            set
            {
                improveBoundaryBehavior = value;
            }
        }


        protected internal ContactEventManager<InstancedMesh> events;
        ///<summary>
        /// Gets the event manager of the mesh.
        ///</summary>
        public ContactEventManager<InstancedMesh> Events
        {
            get
            {
                return events;
            }
            set
            {
                if (value.Owner != null && //Can't use a manager which is owned by a different entity.
                    value != events) //Stay quiet if for some reason the same event manager is being set.
                    throw new Exception("Event manager is already owned by a mesh; event managers cannot be shared.");
                if (events != null)
                    events.Owner = null;
                events = value;
                if (events != null)
                    events.Owner = this;
            }
        }
        protected internal override IContactEventTriggerer EventTriggerer
        {
            get { return events; }
        }

        protected override IDeferredEventCreator EventCreator
        {
            get
            {
                return events;
            }
        }


        /// <summary>
        /// Tests a ray against the entry.
        /// </summary>
        /// <param name="ray">Ray3 to test.</param>
        /// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
        /// <param name="rayHit">Hit location of the ray on the entry, if any.</param>
        /// <returns>Whether or not the ray hit the entry.</returns>
        public override bool RayCast(Ray3 ray, float maximumLength, out RayHit rayHit)
        {
            return RayCast(ray, maximumLength, sidedness, out rayHit);
        }

        ///<summary>
        /// Tests a ray against the instance.
        ///</summary>
        ///<param name="ray">Ray3 to test.</param>
        ///<param name="maximumLength">Maximum length of the ray to test; in units of the ray's direction's length.</param>
        ///<param name="sidedness">Sidedness to use during the ray cast.  This does not have to be the same as the mesh's sidedness.</param>
        ///<param name="rayHit">The hit location of the ray on the mesh, if any.</param>
        ///<returns>Whether or not the ray hit the mesh.</returns>
        public bool RayCast(Ray3 ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
        {
            //Put the ray into local space.
            Ray3 localRay;
            AffineTransform3 inverse;
            AffineTransform3.Invert(ref worldTransform, out inverse);
            Vector3.Transform(ref ray.Direction, ref inverse.Transform, out localRay.Direction);
            Vector3.Transform(ref ray.Position, ref inverse, out localRay.Position);

            if (Shape.TriangleMesh.RayCast(localRay, maximumLength, sidedness, out rayHit))
            {
                //Transform the hit into world space.
                Vector3.Mul(ref ray.Direction, rayHit.T, out rayHit.Location);
                Vector3.Add(ref rayHit.Location, ref ray.Position, out rayHit.Location);
                Vector3.TransformTranspose(ref rayHit.Normal, ref inverse.Transform, out rayHit.Normal);
                return true;
            }
            rayHit = new RayHit();
            return false;
        }

        /// <summary>
        /// Casts a convex shape against the collidable.
        /// </summary>
        /// <param name="castShape">Shape to cast.</param>
        /// <param name="startingTransform">Initial transform of the shape.</param>
        /// <param name="sweep">Sweep to apply to the shape.</param>
        /// <param name="hit">Hit data, if any.</param>
        /// <returns>Whether or not the cast hit anything.</returns>
        public override bool ConvexCast(CollisionShapes.ConvexShapes.ConvexShape castShape, ref RigidTransform3 startingTransform, ref Vector3 sweep, out RayHit hit)
        {
            hit = new RayHit();
            BoundingBox3 boundingBox;
            castShape.GetSweptLocalBoundingBox(ref startingTransform, ref worldTransform, ref sweep, out boundingBox);
            var tri = Resources.GetTriangle();
            var hitElements = Resources.GetIntList();
            if (this.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, hitElements))
            {
                hit.T = float.MaxValue;
                for (int i = 0; i < hitElements.Count; i++)
                {
                    Shape.TriangleMesh.Data.GetTriangle(hitElements[i], out tri.vA, out tri.vB, out tri.vC);
                    Vector3.Transform(ref tri.vA, ref worldTransform, out tri.vA);
                    Vector3.Transform(ref tri.vB, ref worldTransform, out tri.vB);
                    Vector3.Transform(ref tri.vC, ref worldTransform, out tri.vC);
                    Vector3 center;
                    Vector3.Add(ref tri.vA, ref tri.vB, out center);
                    Vector3.Add(ref center, ref tri.vC, out center);
                    Vector3.Mul(ref center, 1f / 3f, out center);
                    Vector3.Sub(ref tri.vA, ref center, out tri.vA);
                    Vector3.Sub(ref tri.vB, ref center, out tri.vB);
                    Vector3.Sub(ref tri.vC, ref center, out tri.vC);
                    tri.maximumRadius = tri.vA.LengthSquared();
                    float radius = tri.vB.LengthSquared();
                    if (tri.maximumRadius < radius)
                        tri.maximumRadius = radius;
                    radius = tri.vC.LengthSquared();
                    if (tri.maximumRadius < radius)
                        tri.maximumRadius = radius;
                    tri.maximumRadius = (float)Math.Sqrt(tri.maximumRadius);
                    tri.collisionMargin = 0;
                    var triangleTransform = new RigidTransform3 { Orientation = Quaternion.Identity, Position = center };
                    RayHit tempHit;
                    if (MPRToolbox.Sweep(castShape, tri, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref triangleTransform, out tempHit) && tempHit.T < hit.T)
                    {
                        hit = tempHit;
                    }
                }
                tri.maximumRadius = 0;
                Resources.GiveBack(tri);
                Resources.GiveBack(hitElements);
                return hit.T != float.MaxValue;
            }
            Resources.GiveBack(tri);
            Resources.GiveBack(hitElements);
            return false;
        }

    
    }
}
