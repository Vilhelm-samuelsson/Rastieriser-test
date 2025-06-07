using System.Numerics;

namespace MainEngine
{
    public class Transform
    {
        public Vector3 postion;
        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 rotation;


        public Vector3 ToWorldPoint(Vector3 point)
        {
            (Vector3 ihat, Vector3 jhat, Vector3 khat) = GetBasisVector();
            return TransformVector(ihat, jhat, khat, point) * scale + postion;
        }

        (Vector3 ihat, Vector3 jhat, Vector3 khat) GetBasisVector()
        {
            Vector3 ihat_yaw = new(MathF.Cos(rotation.Y * 9.425f), 0, MathF.Sin(rotation.Y * 9.425f));
            Vector3 jhat_yaw = new(0, 1, 0);
            Vector3 khat_yaw = new(-MathF.Sin(rotation.Y * 9.425f), 0, MathF.Cos(rotation.Y * 9.425f));

            Vector3 ihat_pitch = new(1, 0, 0);
            Vector3 jhat_pitch = new(0, MathF.Cos(rotation.X * 9.425f), -MathF.Sin(rotation.X * 9.425f));
            Vector3 khat_pitch = new(0, MathF.Sin(rotation.X * 9.425f), MathF.Cos(rotation.X * 9.425f));

            Vector3 ihat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, ihat_pitch);
            Vector3 jhat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, jhat_pitch);
            Vector3 khat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, khat_pitch);

            return (ihat, jhat, khat);
        }

        static Vector3 TransformVector(Vector3 ihat, Vector3 jhat, Vector3 khat, Vector3 v)
        {
            return v.X * ihat + v.Y * jhat + v.Z * khat;
        }
    }
}