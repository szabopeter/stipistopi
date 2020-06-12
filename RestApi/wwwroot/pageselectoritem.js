import { Noop } from "./util.js";

export function PageSelectorItemViewModel(label, componentName, componentVm) {
    this.label = ko.observable(label);
    this.componentName = ko.observable(componentName);
    this.componentVm = ko.observable(componentVm);
    this.activate = Noop;
}
