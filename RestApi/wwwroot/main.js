import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { TemplateManager } from "./templatemanager.js";


function ResourceActions() {
    let internals = {
        actions: [],
    };

    function registerAction(action) {
        action.id = internals.actions.length;
        internals.actions.push(action);
    }

    function update(resources) {
        internals.actions.length = 0;
        resources.forEach(resource => {
            resource.actions.forEach(registerAction);
        });    
    }

    var resourceActions = {
        /**
         * Will be called from resourceline template
         * @param {Number} index 
         */
        executeByNr: function (index) { internals.actions[index].execute(); },
        registerAction: registerAction,
        update: update,
    };

    return resourceActions;
}

function Backend() {
    function sendResourceAction(action, resourceName, onSuccess) {
        let url = "./stipistopi/" + action;
        let lockParameter = {
            resourceName: resourceName,
            user: { userName: "test", password: "test" },
        };
        AjaxPost(url, lockParameter, onSuccess, Noop)
    };

    return {
        sendResourceAction: sendResourceAction
    };
}

function StipiStopi() {
    let stipistopi = {
        resourceActions: ResourceActions(),
        templateManager: TemplateManager(),
        backend: Backend(),
    }

    stipistopi.UpdateResourceList = function UpdateResourceList(resources) {
        let resourceActions = stipistopi.resourceActions;

        // Update internal state (resourceActions)
        function onSuccess() {
            stipistopi.DownloadResourceList();
        }

        function extendResource(resource) {
            function getMainAction(resource) {
                let label = resource.isAvailable ? "Lock" : "Release";
                let backendAction = resource.isAvailable ? "lock" : "release";
                let execute = function () {
                    stipistopi.backend.sendResourceAction(backendAction, resource.shortName, onSuccess);
                };

                return {
                    label: label,
                    backendAction: backendAction,
                    execute: execute,
                    id: -1,
                }
            };

            resource.actions = [getMainAction(resource)];
        }

        resources.forEach(extendResource);
        resourceActions.update(resources);

        // Update UI
        let resourceList = document.getElementById("resourceList");
        resourceList.innerHTML = stipistopi.templateManager.ApplyResourceListTemplate(resources);
        console.log("Setting resourceList inner html...");
        // console.log(resourceList.innerHTML);
        document.getElementById("buttonRefreshResourceList").addEventListener("click", stipistopi.DownloadResourceList);
    };

    stipistopi.DownloadResourceList = function DownloadResourceList() {
        // TODO: on failure
        AjaxLoad("./stipistopi/resources", "json", stipistopi.UpdateResourceList, function () { });
    };

    stipistopi.OnTemplatesLoaded = function OnTemplatesLoaded(contents) {
        console.log("Everything should be loaded by now...");
        // console.log(contents);
        stipistopi.templateManager.Load(contents);
        stipistopi.DownloadResourceList();
    };

    return stipistopi;
}

function PageLoaded() {
    let stipistopi = StipiStopi();
    window.resourceActions = stipistopi.resourceActions;
    console.log("window.resourceActions has been set");
    stipistopi.templateManager.LoadTemplates(stipistopi.OnTemplatesLoaded);
}

// document.addEventListener("DOMContentLoaded", PageLoaded);
window.PageLoaded = PageLoaded;
