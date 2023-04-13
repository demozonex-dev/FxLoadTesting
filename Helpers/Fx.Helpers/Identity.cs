using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Helpers
{
    public static class AuthenticationRecordExtension 
    {
       
        public static async Task PersisteAsync(this AuthenticationRecord record, string path)
        {
            // Serialize the AuthenticationRecord to disk so that it can be re-used across executions of this initialization code.
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
    
    public  class Identity
    {
        const string AUTH_PATH_FILE = "cat.bin";
        const string TOKEN_CACHE_NAME = "laCatToken";
        public static async Task<TokenCredential> BrowserInteractiveAuthenticateAsync()
        {
            InteractiveBrowserCredential? credential=null;
            AuthenticationRecord? authRecord=null;

            if (!File.Exists(AUTH_PATH_FILE))
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
                await authRecord.PersisteAsync(AUTH_PATH_FILE);
                 
            }
            else
            {
               authRecord= await AuthenticationRecordExtension.LoadAsync(AUTH_PATH_FILE);
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
        
        public static async Task<TokenCredential> DeviceCodeAuthenticateAsync()
        {
            DeviceCodeCredential? credential = null;
            AuthenticationRecord? authRecord = null;

            if (!File.Exists(AUTH_PATH_FILE))
            {

                DeviceCodeCredentialOptions options = new DeviceCodeCredentialOptions
                {
                    TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                    {
                        Name = TOKEN_CACHE_NAME
                    }
                };
                credential = new DeviceCodeCredential(options);
                authRecord = await credential.AuthenticateAsync();
                await authRecord.PersisteAsync(AUTH_PATH_FILE);

            }
            else
            {
                authRecord = await AuthenticationRecordExtension.LoadAsync(AUTH_PATH_FILE);
                // Construct a new client with our TokenCachePersistenceOptions with the addition of the AuthenticationRecord property.
                // This tells the credential to use the same token cache in addition to which account to try and fetch from cache when GetToken is called.
                credential = new DeviceCodeCredential(
                    new DeviceCodeCredentialOptions
                    {
                        TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = TOKEN_CACHE_NAME },
                        AuthenticationRecord = authRecord
                    });
            }

            return credential;
        }
       
    }
}
