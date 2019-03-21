using System;

namespace AnAusAutomat.Controllers.Serial.Internals
{
    public static class ByteBuilder
    {
        public static byte[] Hello()
        {
            return new byte[] { Decrypt(Method.Hello, 0) };
        }

        public static byte[] Ping()
        {
            return new byte[] { Decrypt(Method.Ping, 0) };
        }

        public static byte[] TurnOff(int socketId)
        {
            if (socketId < 0 || socketId > 63)
            {
                throw new ArgumentOutOfRangeException("pin", socketId, "Value must be between 0 and 63.");
            }

            return new byte[] { Decrypt(Method.TurnOff, socketId) };
        }

        public static byte[] TurnOn(int socketId)
        {
            if (socketId < 0 || socketId > 63)
            {
                throw new ArgumentOutOfRangeException("pin", socketId, "Value must be between 0 and 63.");
            }

            return new byte[] { Decrypt(Method.TurnOn, socketId) };
        }

        public static byte Decrypt(Method method, int socketId)
        {
            return (byte)((socketId << 2) + method);
        }

        public static Method Encrypt(byte value, out int socketId)
        {
            var method = (Method)(value & 3);
            socketId = (value - (int)method) >> 2;
            return method;
        }
    }
}
