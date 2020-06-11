import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { UnauthenticatedMainContentViewModel, unauthenticatedMainContentRegisterWidget } from "./credentialspage.js"

export function MainWindowViewModel() {
    this.mainContent = ko.observable();
    this.pageSelector = new PageSelectorViewModel();
    this.pageSelectorVm = ko.observable();
    this.unauthenticatedMainContentVm = new UnauthenticatedMainContentViewModel();
    this.authenticatedMainContentVm = new UnauthenticatedMainContentViewModel();
    this.debug = function () {
        console.log("Current state of mainContent:");
        console.log(ko.toJSON(this.mainContent()));
    };

    this.pageSelector.selected = this.mainContent;
}
