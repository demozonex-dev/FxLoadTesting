// See https://aka.ms/new-console-template for more information

using Azure.Identity;
using Fx.ArmManager;

ResourceClient resourceClient = new ResourceClient();
resourceClient.Login(await Fx.Helpers.Identity.AuthenticateAsync(Fx.Helpers.AuthenticationType.DeviceCode));

