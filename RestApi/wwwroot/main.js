import { AjaxLoad } from "./util.js";
import { TemplateManager } from "./templatemanager.js";


function ResourceActions() {
    let internals = {
        actions: [],
    };

    var resourceActions = {
        /**
         * Will be called from resourceline template
         * @param {Number} index 
         */
        executeByNr: function (index) { internals.actions[index].execute(); },
        clear: function () { internals.actions = []; },
        create: function (label, fn) {
            let action = { label: label, execute: fn, id: internals.actions.length };
            internals.actions.push(action);
            return action;
        },
    };

    return resourceActions;
}

function Backend() {
    function sendResourceAction(action, resourceName, onSuccess) {
        let lockParameter = {
            resourceName: resourceName,
            user: { userName: "test", password: "test" },
        };

        let xhttp = new XMLHttpRequest();
        xhttp.responseType = "json";
        xhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                onSuccess();
            }
            // TODO : on failure?
        };
        xhttp.open("POST", "./stipistopi/" + action);
        xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xhttp.send(JSON.stringify(lockParameter));
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
        // Update internal state (resourceActions)
        let resourceActions = stipistopi.resourceActions;
        resourceActions.clear();
        resources.forEach(resource => {
            let label = resource.isAvailable ? "Lock" : "Release";
            let resourceAction = resource.isAvailable ? "lock" : "release";
            function onSuccess() {
                stipistopi.DownloadResourceList();
            }
            let execute = function () {
                stipistopi.backend.sendResourceAction(resourceAction, resource.shortName, onSuccess);
            };
            let action = resourceActions.create(
                label,
                execute
            );
            resource.actions = [action];
        });

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
