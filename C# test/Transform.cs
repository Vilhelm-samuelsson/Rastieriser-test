using System.Numerics;

namespace MainEngine
{
    public class Transform
    {
        public Vector3 postion;
        public Vector3 scale = new Vector3(1, 1, 1);
        public float pitch { get => _pitch; set => setrotation(value, _jaw); }
        public float jaw { get => _jaw; set => setrotation(_pitch, value); }

        float _pitch = 0;
        float _jaw = 0;

        Vector3 ihat;
        Vector3 jhat;
        Vector3 khat;

        Vector3 invihat;
        Vector3 invjhat;
        Vector3 invkhat;


        void setrotation(float x,float y)
        {
            _pitch = x;
            _jaw = y;

            (ihat,jhat, khat) = GetBasisVector();
            (invihat, invjhat, invkhat) = GetInverseBasisVector();
        }

        public Vector3 ToWorldPoint(Vector3 point)
        {
            return TransformVector(ihat, jhat, khat, point) * scale + postion;
        }

        public Vector3 ToLocalPoint(Vector3 point)
        {
            
            return TransformVector(invihat, invjhat, invkhat, point  - postion);
        }

        public Vector3 transformednormal(Vector3 normal)
        {
            (Vector3 ihat, Vector3 jhat, Vector3 khat) = GetBasisVector();
            return TransformVector(ihat, jhat, khat, normal);
        }

        public (Vector3 ihat, Vector3 jhat, Vector3 khat) GetBasisVector()
        {

            Vector3 ihat_pitch = new(1, 0, 0);
            Vector3 jhat_pitch = new(0, MathF.Cos(_pitch), -MathF.Sin(_pitch));
            Vector3 khat_pitch = new(0, MathF.Sin(_pitch), MathF.Cos(_pitch));

            Vector3 ihat_yaw = new(MathF.Cos(_jaw), 0, MathF.Sin(_jaw));
            Vector3 jhat_yaw = new(0, 1, 0);
            Vector3 khat_yaw = new(-MathF.Sin(_jaw), 0, MathF.Cos(_jaw));

            Vector3 ihat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, ihat_pitch);
            Vector3 jhat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, jhat_pitch);
            Vector3 khat = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, khat_pitch);

            return (ihat, jhat, khat);
        }

        public (Vector3 ihat, Vector3 jhat, Vector3 khat) GetInverseBasisVector()
        {

            Vector3 ihat_pitch = new(1, 0, 0);
            Vector3 jhat_pitch = new(0, MathF.Cos(-_pitch), -MathF.Sin(-_pitch));
            Vector3 khat_pitch = new(0, MathF.Sin(-_pitch), MathF.Cos(-_pitch));

            Vector3 ihat_yaw = new(MathF.Cos(-_jaw), 0, MathF.Sin(-_jaw));
            Vector3 jhat_yaw = new(0, 1, 0);
            Vector3 khat_yaw = new(-MathF.Sin(-_jaw), 0, MathF.Cos(-_jaw));

            Vector3 ihat = TransformVector(ihat_pitch, jhat_pitch, khat_pitch, ihat_yaw);
            Vector3 jhat = TransformVector(ihat_pitch, jhat_pitch, khat_pitch, jhat_yaw);
            Vector3 khat = TransformVector(ihat_pitch, jhat_pitch, khat_pitch, khat_yaw);

            return (ihat, jhat, khat);
        }

        static Vector3 TransformVector(Vector3 ihat, Vector3 jhat, Vector3 khat, Vector3 v)
        {
            return v.X * ihat + v.Y * jhat + v.Z * khat;
        }
    }
}