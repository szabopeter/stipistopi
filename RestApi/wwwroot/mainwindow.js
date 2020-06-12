import { PageSelectorViewModel, pageSelectorRegisterWidget } from "./pageselector.js";
import { CredentialsPageViewModel, credentialsPageRegisterWidget } from "./credentialspage.js";
import { ResourcesPageViewModel, resourcesPageRegisterWidget } from "./resourcespage.js";
import { Backend } from "./backend.js";


function ComponentManager() {
    let self = this;
    self.widgets = ko.observableArray([]);
    //function has_loaded(widgetName) {
    //    let is_loaded = self.widgets().indexOf(widgetName) != -1;
    //    console.log("Checking if " + widgetName + " is loaded [read]: " + is_loaded);
    //    return is_loaded;
    //}
    self.has_loadeds = {};
    self.has_loaded = function (widgetName) {
        if (!(widgetName in self.has_loadeds)) {
            self.has_loadeds[widgetName] = ko.observable(false);
        }
        return self.has_loadeds[widgetName];
    };

    self.register_loaded = function (widgetName) {
        self.widgets().push(widgetName);
        //console.log("Registered loading of " + widgetName);
        //console.log(self.widgets());
        if (widgetName in self.has_loadeds) {
            self.has_loadeds[widgetName](true);
        }
    };
}

export function MainWindowViewModel() {
    this.backend = Backend();
    this.componentManager = ko.observable(new ComponentManager());
    this.mainContent = ko.observable();
    this.pageSelector = new PageSelectorViewModel();
    this.pageSelectorVm = ko.observable();
    this.credentialsPageVm = new CredentialsPageViewModel();
    this.resourcesPageVm = new ResourcesPageViewModel();
    this.resourcesPageVm.main = this;
    this.debug = function () {
        console.log("Current state of mainContent:");
        console.log(ko.toJSON(this.mainContent()));
    };

    this.pageSelector.selected = this.mainContent;
}
