using System;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthMiddleware.JwtStuff.Signing
{
    internal class SigningCalculator
    {
        /// <summary>
        /// Ensures token integrity
        /// </summary>
        /// <param name="secret">Secret key for signing token</param>
        /// <param name="encodedHeader"></param>
        /// <param name="encodedPayload"></param>
        /// <param name="algorithm">Algorithm used for signing token</param>
        /// <returns></returns>
        internal string CalculateSignature(string secret, string encodedHeader, string encodedPayload, SigningAlgorithmsSupportedEnum algorithm)
        {
            byte[] encodedArray = null;

            var sb = new StringBuilder();

            sb.Append(encodedHeader);
            sb.Append(".");
            sb.Append(encodedPayload);

            string dataPart = sb.ToString();

            var secretByteArray = ConvertStringToByteArray(secret);
            var dataPartByteArray = ConvertStringToByteArray(dataPart);



            switch (algorithm)
            {
                case SigningAlgorithmsSupportedEnum.HMACSHA256:
                    encodedArray = EncodeHMACSHA256(dataPartByteArray, secretByteArray);
                    break;
                default:
                    encodedArray = EncodeHMACSHA256(dataPartByteArray, secretByteArray);
                    break;
            }

            return ConvertHashedByteArrayTostring(encodedArray);
        }

        private byte[] EncodeHMACSHA256(byte[] value, byte[] secret)
        {
            using (HMACSHA256 encoder = new HMACSHA256(secret))
            {
                return encoder.ComputeHash(value);
            }
        }

        private string ConvertHashedByteArrayTostring(byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "").ToLower();
        }


        private byte[] ConvertStringToByteArray(string val)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(val);
        }

        //private string ConvertByteArrayToString(byte[] val)
        //{
        //    return Encoding.UTF8.GetString(val);
        //}

    }
}
