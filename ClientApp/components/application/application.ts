import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import VueRouter from 'vue-router';


@Component({
    components: {
        TaskEditorComponent: require('../taskeditor/taskeditor.vue.html')
    }
})
export default class ApplicationComponent extends Vue {
    $route: any;
    $http: any;
    Application: any = {} as any;
    showApplicationDialog: boolean = false;
    ShowCreateTaskDialog: boolean = false;

    newApplicationKey: string = "";

    Name: string = "";
    Description: string = "";
    showError: boolean = false;
    errorMessage: string = "";
    isUpdating: boolean = false;

    GetApplication() {
        this.$http.get('Application/Get', {
            params: {
                id: this.applicationID
            }
        }).then((response: any) => {
            this.Application = response.data;

            this.Name = this.Application.name;
            this.Description = this.Application.description;
        }).catch((error: any) => {
            alert(error);
        });
    }


    UpdateApplication() {
        this.showError = false;
        this.isUpdating = true;

        this.$http.post('Application/Update', {
            ID: this.Application.id,
            Name: this.Name,
            Description: this.Description
        }).then((response: any) => {
            this.isUpdating = false;
            this.GetApplication();
        }).catch((error: any) => {
            this.errorMessage = error.response.data;
            this.showError = true;
            this.isUpdating = false;
        });
    }


    get applicationID() {
        return this.$route.params.id;
    }

    mounted() {
        this.GetApplication();
    }
}
