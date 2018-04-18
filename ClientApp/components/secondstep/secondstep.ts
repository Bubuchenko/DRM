import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator'
import VueRouter from 'vue-router';



@Component
export default class TaskSecondStepComponent extends Vue {
    $route: any;
    $http: any;

    Configurations: any = [];
    inProgress: boolean = false;
    selected: number = 0;
    canProceed: boolean = false;
    showError: boolean = false;
    errorMessage: string = "";

    @Prop({ default: false })
    value!: boolean;

    reset() {
        this.Configurations = [];
        this.inProgress = false;
        this.selected = 0;
        this.value = false;
    }

    previous() {
        this.$emit('previous');
    }

    proceed() {
        this.$emit('proceed', this.selected);
    }

    close() {
        this.$emit('close');
    }


    headers: any = [
        { text: '', sortable: false },
        { text: 'Server', value: 'server', },
        { text: 'Database', value: 'database' },
        { text: 'Logon', value: 'server' },
        { text: 'Password', value: 'server' }
    ];


    GetConfigurations() {
        this.$http.get('Configuration/All', {
        }).then((response: any) => {
            this.Configurations = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    };

    mounted() {
        this.GetConfigurations();
    }


}
