import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { ResourceLineViewModel } from "./resourceline.js";


function ResourcesPageViewModel(params) {
    let self = this;
    if (params == null) {
        this.main = null;
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
        this.main = params.main;
        this.resources = params.resources;
    };

    function UpdateResourceList(resources) {
        self.resources(resources.map(function(resource) {
            let vm = ResourceLineViewModel.create(resource);
            vm.updateActions(self.main.backend, self.refresh);
            return vm;
        }));
    };

    this.refresh = function () {
        AjaxLoad("./stipistopi/resources", "json", UpdateResourceList, function () { });
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