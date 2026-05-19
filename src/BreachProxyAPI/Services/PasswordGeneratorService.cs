using System;
using System.Text.Json.Serialization;

namespace API_Proxy.Services
{
    public interface IPasswordGeneratorService
    {
        Task<string> GeneratePasswordAsync(int length, bool includeNumbers, bool includeSpecialChars);
    }

    public class PasswordGeneratorService : IPasswordGeneratorService
    {
        private const string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        private readonly ILogger<PasswordGeneratorService> _logger;

        public PasswordGeneratorService(ILogger<PasswordGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GeneratePasswordAsync(int length, bool includeNumbers, bool includeSpecialChars)
        {
            try
            {
                if (length < 1 || length > 64)
                    throw new ArgumentException("Length must be between 1 and 64");

                // Build character set based on options
                var charSet = Letters;
                if (includeNumbers)
                    charSet += Digits;
                if (includeSpecialChars)
                    charSet += SpecialChars;

                // Generate password locally
                var random = new Random();
                var password = new char[length];

                for (int i = 0; i < length; i++)
                {
                    password[i] = charSet[random.Next(charSet.Length)];
                }

                var generatedPassword = new string(password);
                _logger.LogInformation("Password generated locally - length: {Length}, numbers: {Numbers}, special: {Special}",
                    length, includeNumbers, includeSpecialChars);

                // Return JSON format matching the old API response
                var response = new { random_password = generatedPassword };
                return System.Text.Json.JsonSerializer.Serialize(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating password: {Message}", ex.Message);
                throw;
            }
        }
    }
}
