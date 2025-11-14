// Audio tools for converting base64 to blob URLs for reliable audio playback
window.audioTools = {
    base64ToBlobUrl: function (base64, mimeType) {
        const byteString = atob(base64);
        const len = byteString.length;
        const bytes = new Uint8Array(len);

        for (let i = 0; i < len; i++) {
            bytes[i] = byteString.charCodeAt(i);
        }

        const blob = new Blob([bytes], { type: mimeType });
        return URL.createObjectURL(blob);
    },

    // Helper to revoke blob URLs when done (prevent memory leaks)
    revokeBlobUrl: function (blobUrl) {
        if (blobUrl && blobUrl.startsWith('blob:')) {
            URL.revokeObjectURL(blobUrl);
        }
    }
};
