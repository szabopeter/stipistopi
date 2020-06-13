import { AjaxLoad, AjaxPost, Noop } from "./util.js";


export function Backend() {
    let myUser = {
        userName: "test",
        password: "test",
    };

    function setCredentials(userName, password) {
        myUser.userName = userName;
        myUser.password = password;
    }

    function sendResourceAction(action, resourceName, onSuccess) {
        let url = "./stipistopi/" + action;
        let lockParameter = {
            resourceName: resourceName,
            user: myUser,
        };
        AjaxPost(url, lockParameter, onSuccess, Noop)
    }

    function loadUsers(onSuccess, onFailure) {
        AjaxPost("./stipistopi/users", myUser, onSuccess, onFailure);
    }

    function createUserAction(userName, password, role, onSuccess) {
        let newUserParameter = {
            user: { userName: userName, password: password, role: role },
            creator: myUser,
        };
        // TODO: on failure
        AjaxPost("./stipistopi/register", newUserParameter, onSuccess, Noop)
    }

    return {
        loadUsers: loadUsers,
        sendResourceAction: sendResourceAction,
        createUserAction: createUserAction,
        setCredentials: setCredentials,
    };
}