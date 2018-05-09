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

    @Prop({ default: false })
    value!: boolean;

    stepProgress: number = 1;
    taskIDs: number[] = [];
    taskProgress: any = {};
    intervalHandle: any = {};
    databaseIntervalHandle: any = {};
    databaseStatuses: any = [];
    finishedTasks: any = [];


    @Prop({ default: {} })
    params!: any;

    @Prop({ default: {} })
    configurations!: any;

    @Watch('value')
    onvalueChanged(val: any, oldVal: any) {
        //Trigger for starting the excuter
        if (val == true) {
            setTimeout(this.BackupDatabases, 4000);
        }
    }

    @Watch('stepProgress')
    onstepChanged(val: number, oldVal: number) {
        if (val > 1 && val < this.params.length + 2) {
            this.performTransformations();
        }
        //Marks the end of the process
        else if (val == this.params.length + 2) {
            clearInterval(this.intervalHandle);
        }
    }

    performTransformations() {
        if (this.params.length > 0) {
            this.taskProgress = {};
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

            this.getTaskProgress();
            this.intervalHandle = setInterval(this.getTaskProgress, 3000);
        }
    }

    close() {
        this.$emit('close');

        this.stepProgress = 1;
        this.taskIDs = [];
        this.taskProgress = {};
        this.params = null;
        this.configurations = null;
        this.finishedTasks = [];
        clearInterval(this.intervalHandle);
        clearInterval(this.databaseIntervalHandle);
    }

    getFinishedTasks() {
        for (var i = 0; i < this.taskIDs.length; i++) {
            this.finishedTasks.push()
        }
    }

    checkBackupProgress() {
        var finishedBackups = 0;
        for (var i = 0; i < this.configurations.length; i++) {
            var configuration = this.configurations[i];

            if (configuration.IsComplete)
                finishedBackups++;
        }

        //Database backups finished?
        if (finishedBackups == this.configurations.length) {

            this.stepProgress++; //Proceed
            clearInterval(this.databaseIntervalHandle);
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
            console.log("error:" + error);
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
            ApplicationName: this.params[this.stepProgress - 2].applicationName,
            ID: taskID,
            RecordIDs: records
        }).then((response: any) => {
            //Finished? Next step
            this.getTaskProgress();
            this.stepProgress++;
            this.finishedTasks.push(this.taskProgress);
        }).catch((error: any) => {
            console.log("error:" + error);
        });
    }



    BackupDatabases() {
        //Add an extra prop to each configuration to track completion
        for (var i = 0; i < this.configurations.length; i++) {
            this.configurations[i].IsComplete = false;
        }

        for (var i = 0; i < this.configurations.length; i++) {
            this.backupDatabase(this.configurations[i]);
        }

        this.databaseIntervalHandle = setInterval(this.checkBackupProgress, 500);
    }

    backupDatabase(configuration: any) {
        this.$http.post('Database/Backup', {
            id: configuration.id,
        }).then((response: any) => {
            configuration.IsComplete = true;
        }).catch((error: any) => {
            console.log(error);
        });
    }
}
