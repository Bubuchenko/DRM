import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component
export default class TaskSecondStepComponent extends Vue {
    $route: any;
    $http: any;


    @Prop({ default: false })
    value!: boolean;

    previous() {
        this.$emit('previous');
    }

    proceed() {
        this.$emit('proceed');
    }

    inProgress: boolean = false;
    showError: boolean = false;
    errorMessage: any = "";
    Name: string = "";
    Description: string = "";
    canProceed: boolean = false;

    headers: any = [{
        text: 'Server',
        value: 'Server',
    }, 
    { text: 'Database', value: 'Database' }];

    items: any = [{
        Server: 'WSRV4646',
        Database: 'RIFT'
    }];


}
