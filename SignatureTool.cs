#if !UNITY_WSA || UNITY_EDITOR
using System.Security.Cryptography;
#else
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
#endif
using System;
using UnityEngine;

namespace Runtime
{
    using Runtime.Pattern;

    public class SignatureTool : Singleton<SignatureTool>
    {
        const string PUBLIC_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQA9A4S5rD4yCIzmeBkccQMwK/Sy3RDYdUeukpLuiMeMLNYlQ+jZwUXauxZ6DP8cZ29JlC1DzN9b89WKwzGprdeI6RV+pzQceR1bTdEEgGMQA7fQL+reOdmnBT3B70xRRn9DHufD/zxsOKN+mVoaF5T7VeKOZnse1IISjMgh7RL50w==";

#if !UNITY_WSA || UNITY_EDITOR
        RSACryptoServiceProvider rsa;
        SHA1 sha;
#else
        AsymmetricKeyAlgorithmProvider rsa;
        CryptographicKey key;
#endif

        public SignatureTool()
        {
#if !UNITY_WSA || UNITY_EDITOR
            rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(PUBLIC_KEY));
            sha = new SHA1CryptoServiceProvider();
#else
            rsa = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaSignPkcs1Sha1);
            key = rsa.ImportPublicKey(CryptographicBuffer.DecodeFromBase64String(PUBLIC_KEY), CryptographicPublicKeyBlobType.Capi1PublicKey);
#endif
        }

        public byte[] LoadAndVerify(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            
            if (data.Length < 128)
            {
                throw new InvalidProgramException("data length less than 128!");
            }

            byte[] sig = new byte[128];
            byte[] filecontent = new byte[data.Length - 128];
            Array.Copy(data, sig, 128);
            Array.Copy(data, 128, filecontent, 0, filecontent.Length);

#if !UNITY_WSA || UNITY_EDITOR
            if (!rsa.VerifyData(filecontent, sha, sig))
            {
                throw new InvalidProgramException("data has invalid signature!");
            }
#else
            if (!CryptographicEngine.VerifySignature(key, CryptographicBuffer.CreateFromByteArray(filecontent), CryptographicBuffer.CreateFromByteArray(sig)))
            {
                throw new InvalidProgramException("data has invalid signature!");
            }
#endif
            return filecontent;
        }

        public string GetText(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(LoadAndVerify(data));
        } 
    }
}