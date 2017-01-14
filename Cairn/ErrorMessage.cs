using System;

namespace Cairn {
    public class ErrorMessage {
        private readonly string _message;
        private readonly Exception _exception;

        public ErrorMessage(string message, Exception exception) {
            this._message = message;
            this._exception = exception;
        }

        public string Message {
            get { return _message; }
        }

        public Exception Exception {
            get { return _exception; }
        } 

    }
}
