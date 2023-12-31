# iCat.Crypto

iCat.Crypto integrate some hasher, crypto methods.

## Installation
```bash
dotnet add package iCat.Crypto
```

## Configuration

```C#
    using iCat.Crypto.Interfaces;
    using iCat.Crypto.Implements.Cryptors;
```
```C#
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            var rsaPrivateKey = "fill your RSA private key";
            var rsaPublicKey = "fill your RSA public key";

            // Add services to the container.
            services.AddControllers();
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.AES("AES", "12345678"));
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.DES("DES", "12345678"));
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.RSA("RSA", rsaPublicKey, rsaPrivateKey));
            services.AddSingleton<IHasher>(p => new iCat.Crypto.Implements.Hashes.Hasher("Hash", "12345678"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
```
---
### Sample

```C# Using
    using iCat.Crypto;
    using iCat.Crypto.Interfaces;
```

```C#
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IEnumerable<ICryptor> _cryptors;
        private readonly IHasher _hasher;

        public DemoController(IEnumerable<ICryptor> cryptors, IHasher hasher)
        {
            _cryptors = cryptors ?? throw new ArgumentNullException(nameof(cryptors));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        [HttpGet]
        public IActionResult Get()
        {
            var plainText = "plainText";

            // use by DI
            var cipherTextAES = _cryptors.First(p => p.Category == "AES").Encrypt(plainText);
            var cipherTextDES = _cryptors.First(p => p.Category == "DES").Encrypt(plainText);
            var cipherTextRSA = _cryptors.First(p => p.Category == "RSA").Encrypt(plainText);

            var resultAES = _cryptors.First(p => p.Category == "AES").Decrypt(cipherTextAES);
            var resultDES = _cryptors.First(p => p.Category == "DES").Decrypt(cipherTextDES);
            var resultRSA = _cryptors.First(p => p.Category == "RSA").Decrypt(cipherTextRSA);

            var hashMD5 = _hasher.MD5(plainText);
            var hashSH1 = _hasher.SHA1(plainText);
            var hashSH256 = _hasher.SHA256(plainText);
            var hashSH384 = _hasher.SHA384(plainText);
            var hashSH512 = _hasher.SHA512(plainText);

            // use by static method
            cipherTextAES = AES.Encrypt("your key", plainText);
            cipherTextDES = DES.Encrypt("your key", plainText);
            cipherTextRSA = RSA.Encrypt("your RSA public key", plainText);

            resultAES = AES.Decrypt("your key", plainText);
            resultDES = DES.Decrypt("your key", plainText);
            resultRSA = RSA.Decrypt("your RSA private key", plainText);

            hashMD5 = Hash.MD5("your key", plainText);
            hashSH1 = Hash.SHA1("your key", plainText);
            hashSH256 = Hash.SHA256("your key", plainText);
            hashSH384 = Hash.SHA384("your key", plainText);
            hashSH512 = Hash.SHA512("your key", plainText);

            var signed = RSA.Sign("your RSA private key", plainText);
            var verifyResult = RSA.Verify("your RSA public key", signed, plainText);

            return Ok();
        }
    }
```