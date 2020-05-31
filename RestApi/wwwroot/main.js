import { AjaxLoad, AjaxPost, Noop } from "./util.js";
import { TemplateManager } from "./templatemanager.js";
import { ResourceActions } from "./resourceactions.js"


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

    function extendResource(resource) {
        function getMainAction(resource) {
            let label = resource.isAvailable ? "Lock" : "Release";
            let backendAction = resource.isAvailable ? "lock" : "release";
            let execute = function () {
                stipistopi.backend.sendResourceAction(backendAction, resource.shortName, stipistopi.DownloadResourceList);
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

    stipistopi.UpdateResourceList = function UpdateResourceList(resources) {
        resources.forEach(extendResource);
        stipistopi.resourceActions.update(resources);

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
