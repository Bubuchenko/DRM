import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator'
import VueRouter from 'vue-router';
import moment from 'moment';
import CryptoJS from 'crypto-js';

@Component({
    components: {
        MenuComponent: require('../navmenu/navmenu.vue.html')
    }
})
export default class ExecuterComponent extends Vue {
    $route: any;
    $http: any;

    $document: any;

    @Prop({ default: false })
    value!: boolean;

    stepProgress: number = 1;
    taskIDs: number[] = [];
    taskProgress: any = {};


    @Prop({ default: {} })
    params!: any;

    @Prop({ default: {} })
    configurations!: any;

    @Watch('value')
    onvalueChanged(val: any, oldVal: any) {
        //Trigger for starting the excuter
        if (val == true) {
            //this.BackupDatabases();
        }
    }

    @Watch('stepProgress')
    onstepChanged(val: number, oldVal: number) {
        if (val > 1) {
            this.performTransformations();
        }
    }

    performTransformations() {
        if (this.params.length > 0) {

            //List of all transformations
            var transformations = [];

            //For each task in application
            for (var i = 0; i < this.params[this.stepProgress - 2].nonCompliantRecordSets.length; i++) {
                var task = this.params[this.stepProgress - 2].nonCompliantRecordSets[i];
                //For each record in the task
                for (var x = 0; x < task.records.length; x++) {
                    var record = task.records[x].item1;
                    //Add the transformation
                    transformations.push(record);
                }
            }

            this.transformRecords(transformations);

            setInterval(this.getTaskProgress, 3000);
        }
    }

    getTaskProgress() {
        this.$http.get('Database/GetTaskProgress', {
            params: {
                id: this.taskIDs[this.taskIDs.length - 1]
            }
        }).then((response: any) => {
            this.taskProgress = response.data;
        }).catch((error: any) => {
            console.log("error:" + error.response);
        });
    }

    get progressPercentage() {
        if (this.taskProgress == null)
            return 0;

        return Math.round((this.taskProgress.completedItems / this.taskProgress.totalItems) * 100);
    }

    transformRecords(records: any) {
        var taskID = Math.round((new Date()).getTime() / 1000); //UNIX timestamp as ID
        this.taskIDs.push(taskID);
        this.$http.post('Database/TransformRecords', {
            ID: taskID,
            RecordIDs: records
        }).then((response: any) => {
            //Finished? Next step
            this.stepProgress++;
        }).catch((error: any) => {
            console.log("error:" + error.response);
        });
    }

    BackupDatabases() {
        for (var i = 0; i < this.configurations.length; i++) {
            this.backupDatabase(this.configurations[i]);
        }
    }

    backupDatabase(configuration: any) {
        this.$http.post('Database/Backup', {
            id: configuration.id,
        }).then((response: any) => {
            configuration.backupComplete = response.data;
        }).catch((error: any) => {
        });
    }
}
