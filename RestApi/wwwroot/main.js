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
    }

    function createUserAction(onSuccess) {
        let newUserParameter = {
            user: {userName: "newuser", password: ""},
            creator: {username: "test", password: "test"},
        };
        // TODO: on failure
        AjaxPost("./stipistopi/register", newUserParameter, onSuccess, Noop)
    }

    return {
        sendResourceAction: sendResourceAction,
        createUserAction: createUserAction,
    };
}

function StipiStopi(resourceActions, templateManager) {
    let backend = Backend();

    let stipistopi = {
        // Declarations of public methods, definitions will follow below
        /**
         * Called initially when all templates have been loaded
         * @param contents Hashmap of name -> content
         */
        OnTemplatesLoaded: contents => { },
        /**
         * Issue an ajax request to download resources
         */
        DownloadResourceList: () => { },
        /**
         * Issue an ajax request to download users
         */
        DownloadUserList: () => { },
    };

    function extendResource(resource) {
        function getMainAction(resource) {
            let label = resource.isAvailable ? "Lock" : "Release";
            let backendAction = resource.isAvailable ? "lock" : "release";
            let execute = function () {
                backend.sendResourceAction(backendAction, resource.shortName, stipistopi.DownloadResourceList);
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

    function UpdateResourceList(resources) {
        resources.forEach(extendResource);
        resourceActions.update(resources);

        // Update UI
        let resourceList = document.getElementById("resourceList");
        resourceList.innerHTML = templateManager.ApplyResourceListTemplate(resources);
        console.log("Setting resourceList inner html...");
        // console.log(resourceList.innerHTML);
        document.getElementById("buttonRefreshResourceList").addEventListener("click", stipistopi.DownloadResourceList);
    };

    function CreateUser() {
        backend.createUserAction(stipistopi.DownloadUserList);
    }

    function UpdateUserList(users) {
        users.forEach(function(user){
            user.actions = [{id: 1, label: "Edit"}]
        });
        let userList = document.getElementById("userList");
        userList.innerHTML = templateManager.ApplyUserListTemplate(users);
        document.getElementById("buttonRefreshUserList").addEventListener("click", stipistopi.DownloadUserList);
        document.getElementById("buttonCreateUser").addEventListener("click", CreateUser);
    }

    function DownloadResourceList() {
        // TODO: on failure
        AjaxLoad("./stipistopi/resources", "json", UpdateResourceList, function () { });
    };

    function DownloadUserList() {
        // TODO: on failure
        AjaxPost("./stipistopi/users", {userName: "test", password: "test"}, UpdateUserList, function () { });
    }

    function OnTemplatesLoaded(contents) {
        console.log("Everything should be loaded by now...");
        // console.log(contents);
        templateManager.Load(contents);
        stipistopi.DownloadResourceList();
        stipistopi.DownloadUserList();
    };

    // Some methods need to be exposed
    stipistopi.OnTemplatesLoaded = OnTemplatesLoaded;
    stipistopi.DownloadResourceList = DownloadResourceList;
    stipistopi.DownloadUserList = DownloadUserList;

    return stipistopi;
}

function PageLoaded() {
    window.resourceActions = ResourceActions();
    console.log("window.resourceActions has been set");
    let templateManager = TemplateManager();
    let stipistopi = StipiStopi(window.resourceActions, templateManager);
    templateManager.LoadTemplates(stipistopi.OnTemplatesLoaded);
}

// document.addEventListener("DOMContentLoaded", PageLoaded);
window.PageLoaded = PageLoaded;
