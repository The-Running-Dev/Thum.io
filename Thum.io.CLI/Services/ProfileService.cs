using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using PeanutButter.INIFile;

using Thum.io.CLI.Models;
using Thum.io.CLI.Interfaces;

namespace Thum.io.CLI.Services
{
    public class ProfileService : IProfileService
    {
        public static string ConfigDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create), _configDirName);

        public static string ConfigFile => Path.Combine(ConfigDir, _configFileName);

        private static readonly string _configDirName = ".thum.io";

        private static readonly string _configFileName = "profile.ini";

        private readonly IINIFile _iniFile;

        public ProfileService(IINIFile iniFile)
        {
            _iniFile = iniFile;
        }

        public Profile Get()
        {
            if (!_iniFile.Sections.Any())
            {
                return null;
            }

            var section = _iniFile.Sections.FirstOrDefault();
            var profile = new Profile
            {
                Name = section,
                ApiKey = Decrypt(_iniFile[section][nameof(Profile.ApiKey)])
            };

            return profile;
        }

        public Task Create(string apiKey, string name = "default")
        {
            if (apiKey.IsEmpty())
            {
                return null;
            }

            Persist(new Profile
            {
                Name = name,
                ApiKey = Encrypt(apiKey)
            });

            return Task.CompletedTask;
        }

        public void Update(Profile profile)
        {
            if (profile.ApiKey.IsEmpty())
            {
                return;
            }

            profile.ApiKey = Encrypt(profile.ApiKey);

            Persist(profile);
        }

        private void Persist(Profile profile)
        {
            if (!_iniFile.HasSection(profile.Name))
            {
                _iniFile.AddSection(profile.Name);
            }

            _iniFile[profile.Name][nameof(Profile.ApiKey)] = profile.ApiKey;
            _iniFile.Persist();
        }

        private string EncryptKey
        {
            get
            {
                var keyLen = 32;
                var key = Environment.UserName;

                if (key.Length > keyLen)
                {
                    key = key.Substring(0, keyLen);
                }
                else if (key.Length < keyLen)
                {
                    var len = key.Length;

                    for (var i = 0; i < keyLen - len; i++)
                    {
                        key += ((char)(65 + i)).ToString();
                    }
                }

                return key;
            }
        }

        protected string Encrypt(string text)
        {
            var key = Encoding.UTF8.GetBytes(EncryptKey);

            using var aes = Aes.Create();
            using var transform = aes.CreateEncryptor(key, aes.IV);
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))

            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(text);
            }

            var iv = aes.IV;
            var decryptedContent = memoryStream.ToArray();
            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }

        protected string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            var key = Encoding.UTF8.GetBytes(EncryptKey);

            using var aes = Aes.Create();
            using var transform = aes.CreateDecryptor(key, iv);
            using var memoryStream = new MemoryStream(cipher);
            using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            var result = streamReader.ReadToEnd();

            return result;
        }
    }
}