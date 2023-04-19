using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Helpers
{
    public static class AuthenticationRecordExtension 
    {
       
        public static async Task PersisteAsync(this AuthenticationRecord record, string path)
        {
            // Serialize the AuthenticationRecord to disk so that it can be re-used across executions of this initialization code.
            // TODO : See how to persit file in linux
            
                using var authRecordStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await record.SerializeAsync(authRecordStream);
            
            
        }
        public static async Task<AuthenticationRecord> LoadAsync(string path)
        {
            
            // Load the previously serialized AuthenticationRecord from disk and deserialize it.
            using var authRecordStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return  await AuthenticationRecord.DeserializeAsync(authRecordStream);
        }
    }
    
    public enum AuthenticationType
    {
        DeviceCode,
        BrowserInteractive
    }
    public  class Identity
    {
        const string AUTH_FILE = "FxLoadTestInCat.bin";
        const string TOKEN_CACHE_NAME = "laCatToken";
     
        static string? _pathFile;
        public static string CombinePathFile()
        {
            // Build the auth path file
            // Cross platform  c:\users (Windows) /home/[User] Linux
            //string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string? userPath=null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                userPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            else
            {
                userPath=Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            if (userPath == null) { throw new NullReferenceException(nameof(userPath));}

            return Path.Combine(userPath, AUTH_FILE);
        }
        public static async Task<TokenCredential> AuthenticateAsync(AuthenticationType type=AuthenticationType.DeviceCode)
        {
            
            _pathFile = CombinePathFile();
            
            
            TokenCredential? credential = null;
            switch (type)
            {
                case AuthenticationType.DeviceCode:
                    
                    credential = await DeviceCodeAuthenticateAsync();
                    break;
                case AuthenticationType.BrowserInteractive:
                    credential = await BrowserInteractiveAuthenticateAsync();
                    break;
                default:
                    break;
            }
            if (credential == null) { throw new NullReferenceException(nameof(credential)); }
            return credential;
        }
        
        internal static async Task<TokenCredential> BrowserInteractiveAuthenticateAsync()
        {
            InteractiveBrowserCredential? credential=null;
            AuthenticationRecord? authRecord=null;
            
            if (!File.Exists(_pathFile))
            {

                InteractiveBrowserCredentialOptions options = new InteractiveBrowserCredentialOptions
                {
                    TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                    {                        
                        Name= TOKEN_CACHE_NAME
                    }                    
                };
                credential = new InteractiveBrowserCredential(options);
                authRecord = await credential.AuthenticateAsync();
                await authRecord.PersisteAsync(_pathFile);
                 
            }
            else
            {
               authRecord= await AuthenticationRecordExtension.LoadAsync(_pathFile);
                // Construct a new client with our TokenCachePersistenceOptions with the addition of the AuthenticationRecord property.
                // This tells the credential to use the same token cache in addition to which account to try and fetch from cache when GetToken is called.
                credential = new InteractiveBrowserCredential(
                    new InteractiveBrowserCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = TOKEN_CACHE_NAME },
                        AuthenticationRecord = authRecord
                    });
            }
            
            return credential;
        }
        
        internal static async Task<TokenCredential> DeviceCodeAuthenticateAsync()
        {
            DeviceCodeCredential? credential = null;
            AuthenticationRecord? authRecord = null;
            DeviceCodeCredentialOptions? options = null;
            if (!File.Exists(_pathFile))
            {

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    options = new DeviceCodeCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                        {
                            Name = TOKEN_CACHE_NAME
                        }
                    };
                    
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    
                    options = new DeviceCodeCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                        {
                            Name = "linux"
                        }
                    };
                 
                }
                
                credential = new DeviceCodeCredential(options);
                if (credential == null) { throw new ArgumentNullException(nameof(credential)); }
                
                authRecord = await credential.AuthenticateAsync();
                
                if (authRecord == null) { throw new NullReferenceException(nameof(authRecord)); }
                if (_pathFile == null) { throw new NullReferenceException(nameof(_pathFile)); }
                
                await authRecord.PersisteAsync(_pathFile);
                
                

            }
            else
            {                
                authRecord = await AuthenticationRecordExtension.LoadAsync(_pathFile);
                // Construct a new client with our TokenCachePersistenceOptions with the addition of the AuthenticationRecord property.
                // This tells the credential to use the same token cache in addition to which account to try and fetch from cache when GetToken is called.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    credential = new DeviceCodeCredential(
                    new DeviceCodeCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = TOKEN_CACHE_NAME },
                        AuthenticationRecord = authRecord
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    //TODO : See why the token cache does't work with linux
                    Console.WriteLine("linux");
                    credential = new DeviceCodeCredential(
                    new DeviceCodeCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = "linux" },
                        AuthenticationRecord = authRecord
                    });
                }
                    
                
            }

            return credential;
        }
       
    }
}
