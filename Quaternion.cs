
using System.Collections.Generic;
using Microsoft.Kinect;
using System;

namespace Riken
{

    public static class Quaternion
    {

        public static Vector4 Id()
        {
            return new Vector4() { W = 1, X = 0, Y = 0, Z = 0 };
        }

        public static Vector4 Prod(this Vector4 lhs, Vector4 rhs)
        {
            return new Vector4()
            {
                W = lhs.W * rhs.W  -  lhs.X * rhs.X  -  lhs.Y * rhs.Y  -  lhs.Z * rhs.Z,
                X = lhs.W * rhs.X  +  lhs.X * rhs.W  +  lhs.Y * rhs.Z  -  lhs.Z * rhs.Y,
                Y = lhs.W * rhs.Y  -  lhs.X * rhs.Z  +  lhs.Y * rhs.W  +  lhs.Z * rhs.X,
                Z = lhs.W * rhs.Z  +  lhs.X * rhs.Y  -  lhs.Y * rhs.X  +  lhs.Z * rhs.W,
            };
        }

        public static Vector4 Conj(this Vector4 q)
        {
            return new Vector4()
            {
                W = q.W,
                X = -q.X,
                Y = -q.Y,
                Z = -q.Z
            };
        }

        public static IEnumerable<float> Coords(this Vector4 v)
        {
            yield return v.W;
            yield return v.X;
            yield return v.Y;
            yield return v.Z;
        }

        public static float Norm2(this Vector4 v)
        {
            float res = 0;
            foreach (var vi in v.Coords())
            {
                res += vi * vi;
            }
            return res;
        }

        public static float Norm(this Vector4 v)
        {
            return (float) Math.Sqrt(v.Norm2());
        }

        public static Vector4 Scalar(this Vector4 q, float lambda)
        {
            return new Vector4()
            {
                W = lambda * q.W,
                X = lambda * q.X,
                Y = lambda * q.Y,
                Z = lambda * q.Z
            };
        }

        public static Vector4 Inv(this Vector4 q)
        {
            return q.Conj().Scalar( 1 / q.Norm2() );
        }

        public static Vector4 Flip(this Vector4 q)
        {
            return (q.W < 0) ? q.Scalar(-1) : q;
        }

        public static float Angle(this Vector4 q)
        {
            var w = q.W < 0 ? -q.W : q.W;
            w = w > 1 ? 1 : w;

            return (float)(Math.Acos(w) * 2);
        }

        public const float DEG = (float)(Math.PI / 180.0);
        public const float RAD = (float)(180.0 / Math.PI);

    };


}