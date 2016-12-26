app.constant('NP_IVSwitch_RtpModeEnum', {
    AdvancedProxying: { value: 1, description: "Advanced Proxying - (Possible transcoding)" },
    PassthruProxying: { value: 2, description: "Passthru Proxying - Forwarding only (Packet in / Packet out)" },
    NoProxying: { value: 3, description: "No Proxying - Bypassing (Media flows direct between Endpoints & Routes)" }
});
