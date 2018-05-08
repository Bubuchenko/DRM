 import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import VueRouter from 'vue-router';



@Component({
    components: {
        NewApplicationComponent: require('../newapplication/newapplication.vue.html')
    }
})
export default class DashboardComponent extends Vue {
    Applications: any[] = [];
    ShowCreateApplicationDialog: boolean = false;
    $http: any;

    GetApplications() {
        this.$http.get('Application/All', {
        }).then((response: any) => {
            this.Applications = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    };

    mounted() {
        this.GetApplications();
    }
}
