 import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import VueRouter from 'vue-router';



@Component
export default class OperationsComponent extends Vue {
    $http: any;
    runningTasks: any = [];


    GetTasks() {
        this.$http.get('Database/RunningTasks', {
        }).then((response: any) => {
            this.runningTasks = response.data;
        }).catch((error: any) => {
            alert(error);
        });
    };

    mounted() {
        this.GetTasks();
        setInterval(this.GetTasks, 3000);
    }
}
