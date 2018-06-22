app.constant('VR_Integration_CompressionEnum', {
    False: { value: 0, description: "False" },
    True: { value: 1, description: "True" }
});
app.constant('VR_Integration_SshEncryptionAlgorithmEnum', {
    None: { value: 0, description: "None" },
    RC4: { value: 1, description: "RC4" },
    TripleDES: { value: 2, description: "TripleDES" },
    AES: { value: 4, description: "AES" },
    Blowfish: { value: 8, description: "Blowfish" },
    Twofish: { value: 16, description: "Twofish" },
    Any: { value: 255, description: "Any" }
});
app.constant('VR_Integration_SshHostKeyAlgorithmEnum', {
    None: { value: 0, description: "None" },
    RSA: { value: 1, description: "RSA" },
    DSS: { value: 2, description: "DSS" },
    Any: { value: 255, description: "Any" }
});
app.constant('VR_Integration_SshKeyExchangeAlgorithmEnum', {
    None: { value: 0, description: "None" },
    DiffieHellmanGroup1SHA1: { value: 1, description: "DiffieHellmanGroup1SHA1" },
    DiffieHellmanGroup14SHA1: { value: 2, description: "DiffieHellmanGroup14SHA1" },
    DiffieHellmanGroupExchangeSHA1: { value: 4, description: "DiffieHellmanGroupExchangeSHA1" },
    Any: { value: 255, description: "Any" }
});
app.constant('VR_Integration_SshMacAlgorithmEnum', {
    None: { value: 0, description: "GZ" },
    MD5: { value: 1, description: "MD5" },
    SHA1: { value: 2, description: "SHA1" },
    Any: { value: 255, description: "Any" }
});
app.constant('VR_Integration_SshOptionsEnum', {
    None: { value: 0, description: "None" }
});
