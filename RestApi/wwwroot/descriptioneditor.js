export function DescriptionEditorViewModel(backend, messageManagerVm, updateButtonsState) {
    let self = this;
    this.backend = backend;
    this.messageManagerVm = messageManagerVm;
    this.updateButtonsState = updateButtonsState;
    this.resourceName = "";
    this.oldDescription = ko.observable("");
    this.newDescription = ko.observable("");
    this.isEditing = ko.observable(false);
    this.actions = ko.observableArray();
    this.startEditing = function () { self.isEditing(true); self.setActions(true); self.updateButtonsState(); };
    this.cancelEditing = function () { self.isEditing(false); self.setActions(false); self.updateButtonsState(); };
    this.startSaving = function () {
        backend.updateResourceDescription(
            self.resourceName, self.oldDescription(), self.newDescription(),
            self.saveSucceeded, self.saveFailed
        );
    };
    this.saveFailed = function () { };
    this.saveSucceeded = function (serverResource) {
        console.log("Server responded:");
        console.log(serverResource);
        let descriptionOnServer = serverResource.description;
        self.oldDescription(descriptionOnServer);
        if (descriptionOnServer != self.newDescription()) {
            self.messageManagerVm.showMessageWithOk("Sorry, someone else made conflicting changes. Please check and try again.");
        }
        else {
            self.cancelEditing();
        }
    };
    this.setActions = function (isEditing) {
        if (!self.isEditing()) {
            self.actions([
                { label: "Edit description", callback: self.startEditing }
            ]);
        }
        else {
            self.actions([
                { label: "Cancel editing", callback: self.cancelEditing },
                { label: "Save description", callback: self.startSaving }
            ]);
        }
    };
    this.cancelEditing();
}
