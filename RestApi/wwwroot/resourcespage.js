import { ResourceLineViewModel } from "./resourceline.js";


function ResourcesPageViewModel(params) {
    if (params == null) {
        this.resources = ko.observableArray([
            ResourceLineViewModel.create(
                {
                    shortName: "res1", address: "localhost", isAvailable: false, lockedBy: "some user",
                    actions: [{
                        label: "TODO: releaseAction",
                        action: function () {
                            console.log("TODO: releaseAction");
                            this.isAvailable(!this.isAvailable());
                        }
                    }]
                }),
            ResourceLineViewModel.create(
                {
                    shortName: "res2", address: "remotehost", isAvailable: true, lockedBy: null,
                    actions: [{
                        label: "TODO: lockAction",
                        action: function () {
                            console.log("TODO: lockAction");
                            this.isAvailable(!this.isAvailable());
                        }
                    }]
                }),
            ]);
    } else {
        this.resources = params.resources;
    };

    this.refresh = function () {
        console.log("TODO1: refresh...");
    };
}

function resourcesPageRegisterWidget(template) {
    let widgetName = "resources-page-widget";
    ko.components.register(widgetName, {
        viewModel: ResourcesPageViewModel,
        template: template,
    });
    return widgetName;
}

export { ResourcesPageViewModel, resourcesPageRegisterWidget };