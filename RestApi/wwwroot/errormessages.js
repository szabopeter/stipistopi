function ErrorMessage(message) {
    this.message = ko.observable(message);
    let time = new Date().toLocaleTimeString("en-US", {hour12: false});
    this.time = ko.observable(time);
}

export function ErrorMessagesViewModel() {
    let self = this;
    this.errorMessages = ko.observableArray();
    this.addError = function (error) {
        let message = "";
        if (error == null) {
            message = "Unknown error. The server may not be available.";
        } else if ("message" in error) {
            message = error.message;
        } else if ("Message" in error) {
            message = error.Message;
        } else {
            message = JSON.stringify(error);
        }

        let errorMessage = new ErrorMessage(message);
        self.errorMessages.unshift(errorMessage);
    }
}