using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class Transaction
{
    public string Sender { get; }
    public string Receiver { get; }
    public decimal Amount { get; }
    public string SenderPublicKey { get; }
    public byte[] Signature { get; }

    public Transaction(string sender, string receiver, decimal amount, string senderPublicKey, string privateKey)
    {
        Sender = sender;
        Receiver = receiver;
        Amount = amount;
        SenderPublicKey = senderPublicKey;

        // Sign the transaction
        using (RSA rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey);
            RSAPKCS1SignatureFormatter signatureFormatter = new RSAPKCS1SignatureFormatter(rsa);
            signatureFormatter.SetHashAlgorithm("SHA256");

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{Sender}-{Receiver}-{Amount}"));
            Signature = signatureFormatter.CreateSignature(hash);
        }
    }

    public bool VerifySignature()
    {
        using (RSA rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(SenderPublicKey);
            RSAPKCS1SignatureDeformatter signatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            signatureDeformatter.SetHashAlgorithm("SHA256");

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{Sender}-{Receiver}-{Amount}"));

            return signatureDeformatter.VerifySignature(hash, Signature);
        }
    }

    public override string ToString()
    {
        return $"{{ Sender: {Sender}, Receiver: {Receiver}, Amount: {Amount}, SignatureValid: {VerifySignature()} }}";
    }
}
