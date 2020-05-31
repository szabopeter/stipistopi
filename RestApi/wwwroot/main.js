import { AjaxLoad } from "./util.js";
import { TemplateManager } from "./templatemanager.js";


function ResourceActions() {
    var resourceActions = {
        actions: []
    };
    resourceActions.execute = function (index) { resourceActions.actions[index].execute(); };
    resourceActions.clear = function () { resourceActions.actions = []; };
    resourceActions.create = function (label, fn) {
        let action = { label: label, execute: fn, id: resourceActions.actions.length };
        resourceActions.actions.push(action);
        return action;
    };
    return resourceActions;
}

function StipiStopi() {
    let templateManager = TemplateManager();
    let stipistopi = {
        resourceActions: ResourceActions(),
        templateManager: templateManager,
    }

    stipistopi.SendResourceAction = function SendResourceAction(action, resourceName) {
        let lockParameter = {
            resourceName: resourceName,
            user: { userName: "test", password: "test" },
        };

        let xhttp = new XMLHttpRequest();
        xhttp.responseType = "json";
        xhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                stipistopi.DownloadResourceList();
            }
            // TODO : on failure?
        };
        xhttp.open("POST", "./stipistopi/" + action);
        xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xhttp.send(JSON.stringify(lockParameter));
    };

    stipistopi.UpdateResourceList = function UpdateResourceList(resources) {
        let resourceActions = stipistopi.resourceActions;
        resourceActions.clear();
        resources.forEach(resource => {
            let label = resource.isAvailable ? "Lock" : "Release";
            let resourceAction = resource.isAvailable ? "lock" : "release";
            let execute = function () { stipistopi.SendResourceAction(resourceAction, resource.shortName); };
            let action = resourceActions.create(
                label,
                execute
            );
            resource.actions = [action];
        });
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
