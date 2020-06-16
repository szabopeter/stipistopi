import { AjaxLoad, AjaxPost, Noop } from "./util.js";


export function Backend(errorMessagesVm) {
    let self = this;
    this.errorMessagesVm = errorMessagesVm;

    function createAjaxErrorHandler(callBack)
    {
        return function ajaxErrorHandler(error) {
            self.errorMessagesVm.addError(error);
            callBack(error);
        }
    }

    let myUser = {
        userName: "test",
        password: "test",
    };

    function setCredentials(userName, password) {
        myUser.userName = userName;
        myUser.password = password;
    }

    function sendResourceAction(action, resourceName, onSuccess, onFailure) {
        let url = "./stipistopi/" + action;
        let lockParameter = {
            resourceName: resourceName,
            user: myUser,
        };
        AjaxPost(url, lockParameter, onSuccess, createAjaxErrorHandler(onFailure))
    }

    function loadUsers(onSuccess, onFailure) {
        AjaxPost("./stipistopi/users", myUser, onSuccess, createAjaxErrorHandler(onFailure));
    }

    function createUserAction(userName, password, role, onSuccess, onFailure) {
        let newUserParameter = {
            user: { userName: userName, password: password, role: role },
            creator: myUser,
        };
        AjaxPost("./stipistopi/register", newUserParameter, onSuccess, createAjaxErrorHandler(onFailure));
    }

    return {
        loadUsers: loadUsers,
        sendResourceAction: sendResourceAction,
        createUserAction: createUserAction,
        setCredentials: setCredentials,
    };
}