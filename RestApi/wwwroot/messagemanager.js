export function MessageManagerViewModel() {
    let self = this;
    this.message = ko.observable();

    this.runningTimeout = null;
    this.showMessageWithOk = function (message) {
        window.alert(message);
    };
    this.showDisappearingMessage = function (message) {
        self.message(message);
        if (self.runningTimeout != null)
            window.clearTimeout(self.runningTimeout);

        self.runningTimeout = window.setTimeout(function () {
            self.message("");
            self.runningTimeout = null;
        }, 2500);
    };

    this.showMessage = function (message, disappearing) {
        let showMessage = (disappearing == true) ? showDisappearingMessage : showMessageWithOk;
        showMessage(message);
    };
}
