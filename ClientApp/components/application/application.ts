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
    //Application: Application = "";

    GetApplication() {
        fetch('Application/Get/${this.applicationID}')
            .then(response => response.json() as Promise<Application>)
            .then(data => {
                alert(data);
               console.log(data);
            });
    };

    get applicationID() {
        return this.$route.params.id;
    }


    mounted() {
        this.GetApplication();
    }
}
