export function ResourceActions() {
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
