mergeInto(LibraryManager.library, {
  CopyToClipboard: function (textPtr) {
    const text = UTF8ToString(textPtr);
    
    if (navigator.clipboard && window.isSecureContext) {
      navigator.clipboard.writeText(text).then(function () {
        console.log("Copied to clipboard successfully!");
      }).catch(function (err) {
        console.error("Failed to copy: ", err);
      });
    } else {
      const textArea = document.createElement("textarea");
      textArea.value = text;
      textArea.style.position = "fixed";  // Avoid scrolling to bottom
      document.body.appendChild(textArea);
      textArea.focus();
      textArea.select();

      try {
        document.execCommand('copy');
        console.log("Copied using execCommand");
      } catch (err) {
        console.error("execCommand error: ", err);
      }

      document.body.removeChild(textArea);
    }
  }
});
