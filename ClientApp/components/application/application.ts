import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import VueRouter from 'vue-router';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component
export default class ApplicationComponent extends Vue {
    $route: any;
    $http: any;
    Application: Application = {} as any;
    showApplicationDialog: boolean = false;

    GetApplication() {
        this.$http.get('Application/Get', {
            params: {
                id: this.applicationID
            }
        }).then((response: any) => {
            console.log(response);
            this.Application = response.data;
        }).catch(function (error: any) {
            alert(error);
        });
    }

    get applicationID() {
        return this.$route.params.id;
    }


    async mounted() {
        this.GetApplication();
    }
}
