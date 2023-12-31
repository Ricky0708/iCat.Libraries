using iCat.Crypto.Interfaces;
using iCat.Crypto.Implements.Cryptors;
using System.Security.Cryptography;
using iCat.Crypto.Implements.Hashes;

namespace iCat.Crypto.demo
{
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
}
