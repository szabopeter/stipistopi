import { AjaxLoad, AjaxPost, Noop } from "./util.js";


export function Backend(errorMessagesVm) {
    let self = this;
    this.errorMessagesVm = errorMessagesVm;

    function createAjaxErrorHandler(callBack) {
        return function ajaxErrorHandler(error) {
            self.errorMessagesVm.addError(error);
            callBack(error);
        }
    }

    let myUser = {
        userName: "test",
        password: "test",
    };

    this.setCredentials = function (userName, password) {
        myUser.userName = userName;
        myUser.password = password;
    };

    this.sendResourceAction = function (action, resourceName, onSuccess, onFailure) {
        let url = "./stipistopi/" + action;
        let lockParameter = {
            resourceName: resourceName,
            user: myUser,
        };
        AjaxPost(url, lockParameter, onSuccess, createAjaxErrorHandler(onFailure))
    }

    this.updateResourceDescription = function (resourceName, oldDescription, newDescription, onSuccess, onFailure) {
        let url = "./stipistopi/resource/description";
        let parameter = {
            resourceName: resourceName,
            oldDescription: oldDescription,
            newDescription: newDescription,
            user: myUser,
        };
        AjaxPost(url, parameter, onSuccess, createAjaxErrorHandler(onFailure));
    }

    this.loadResources = function (onSuccess, onFailure) {
        AjaxLoad("./stipistopi/resources", "json", onSuccess, createAjaxErrorHandler(onFailure));
    }

    this.loadUsers = function (onSuccess, onFailure) {
        AjaxPost("./stipistopi/users", myUser, onSuccess, createAjaxErrorHandler(onFailure));
    }

    self.createUserAction = function (userName, password, role, onSuccess, onFailure) {
        let newUserParameter = {
            user: { userName: userName, password: password, role: role },
            creator: myUser,
        };
        AjaxPost("./stipistopi/register", newUserParameter, onSuccess, createAjaxErrorHandler(onFailure));
    }
}