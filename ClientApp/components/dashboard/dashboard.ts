import Vue from 'vue';
import { Component } from 'vue-property-decorator';

interface Application {
    ID: string;
    Name: string;
    Description: string;
    CreationDate: number;
    Tasks: any;
}


@Component
export default class DashboardComponent extends Vue {
    Applications: Application[] = [];

    GetApplications() {
        fetch('Application/All')
            .then(response => response.json() as Promise<Application[]>)
            .then(data => {
                this.Applications = data;
            });
    };


    mounted() {
        this.GetApplications();
    }
}
