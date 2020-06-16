function ErrorMessage(message) {
    this.message = ko.observable(message);
}

export function ErrorMessagesViewModel() {
    let self = this;
    this.errorMessages = ko.observableArray();
    this.addError = function(error) {
        let message = "message" in error ? error.message : JSON.stringify(error);
        let errorMessage = new ErrorMessage(message);
        self.errorMessages.unshift(errorMessage);
    }
}