import { AjaxLoad } from "./util.js";

function TemplateManager() {
    let names = {
        resourceLine: "resourceline.hbs",
        resourceList: "resourcelist.hbs",
    };

    function DownloadTemplate(filename, loadAction, failAction) {
        AjaxLoad(filename, "text", loadAction, failAction);
    }
    
    /**
     * Will call loadAction with all the loaded templates in a hashmap
     * - loads all templates
     * - collects them in a hashmap
     * - calls loadAction when all of them have been loaded, passing the hashmap
     * @param {string[]} templateFiles 
     * @param {function(contents)} loadAction 
     */
    function LoadTemplates(templateFiles, loadAction, failAction) {
        let contents = {};
        let toLoadCount = templateFiles.length;
        templateFiles.forEach(function (fn) {
            let filename = fn;
            DownloadTemplate(filename, function (content) {
                contents[filename] = content;
                console.log("stored " + filename)
                toLoadCount -= 1;
    
                if (toLoadCount == 0)
                    loadAction(contents);
            }, failAction);
        });
    }

    let fileNames = []
    for (let key in names) {
        fileNames.push(names[key]);
    }

    let internals = {
        resourceListTemplate: null,
        // templates: {},
    }

    let templateManager = {
        ApplyResourceListTemplate: function(data) { return internals.resourceListTemplate(data);},
        LoadTemplates: function(loadAction) {
            // TODO: on failure
            LoadTemplates(fileNames, loadAction, function () { });
        },
        Load: function (contents) {
            Handlebars.registerPartial("resourceLineInList", contents[names.resourceLine]);
            internals.resourceListTemplate = Handlebars.compile(contents[names.resourceList]);
            // TODO : there is probably no need for this
            // internals.templates = contents;
        },
    };

    return templateManager;
}

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
