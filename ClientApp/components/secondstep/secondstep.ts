import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component
export default class TaskSecondStepComponent extends Vue {
    $route: any;
    $http: any;

    Configurations: any = [];

    @Prop({ default: false })
    value!: boolean;

    previous() {
        this.$emit('previous');
    }

    proceed() {
        this.$emit('proceed', this.selected);
    }

    inProgress: boolean = false;
    showError: boolean = false;
    errorMessage: any = "";
    Name: string = "";
    Description: string = "";
    canProceed: boolean = false;
    selected: number = 0;

    headers: any = [
        { text: '', sortable: false },
        { text: 'Server', value: 'server', }, 
        { text: 'Database', value: 'database' },
        { text: 'Logon', value: 'server' },
        { text: 'Password', value: 'server' }
    ];


    GetApplications() {
        fetch('Configuration/All')
            .then(response => response.json())
            .then(data => {
                this.Configurations = data;
            });
    };

    mounted() {
        this.GetApplications();
    }


}
