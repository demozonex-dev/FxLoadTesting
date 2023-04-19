param ([string][Parameter( Mandatory=$true)]$receiver)
$receiverPath="I:\repos-1\FxLoadTesting\Receiver\Fx.Receiver\bin\Debug\net7.0\Fx.Receiver.Console.dll"

dotnet $receiverPath $receiver