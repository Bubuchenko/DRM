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

    @Prop({ default: {} })
    params!: any;

    @Prop({ default: {} })
    configurations!: any;

    @Watch('value')
    onvalueChanged(val: any, oldVal: any) {
        //Trigger for starting the excuter
        if (val == true) {
            this.setupApplicationStates();
            //this.BackupDatabases();
        }
    }

    @Watch('stepProgress')
    onstepChanged(val: number, oldVal: number) {
        if (val > 1) {
            this.performTransformations();        
        }
    }

    applicationsStates: any = [];

    setupApplicationStates() {
        for (var i = 0; i < this.params.length; i++) {
            var application = this.params[i];
            var applicationState = {
                name: application.applicationName,
                transformations: [],
                index: 0
            };

            this.applicationsStates.push(applicationState);
        }

        this.stepProgress++;
    }
    
    performTransformations() {
        if (this.params.length > 0) {
            var state = this.applicationsStates[this.stepProgress - 2];
            //For each task in application
            for (var i = 0; i < this.params[this.stepProgress - 2].nonCompliantRecordSets.length; i++) {
                var task = this.params[this.stepProgress - 2].nonCompliantRecordSets[i];

                //For each record in the task
                for (var x = 0; x < task.records.length; x++) {
                    var record = task.records[x];
                    state.transformations.push(record);
                }
            }

            //Apply the actual transformations
            for (var i = 0; i < state.transformations.length; i++) {
                this.transformRecord(state.transformations[i]);
            }
        }
    }


    get transformationProgress(): any {
        var state = this.applicationsStates[this.stepProgress - 2];

        return state != null ? state.index + " out of " + state.transformations.length : "";
    }

    get transformationProgressPercentage(): any {
        var state = this.applicationsStates[this.stepProgress - 2];

        if (state == null)
            return 0;

        return (state.index / state.transformations.length) * 100;
    }

    get totalTransformations(): any {
        return this.applicationsStates[this.stepProgress - 2] != null ? this.applicationsStates[this.stepProgress - 2].index : ""
    }

    BackupDatabases() {
        for (var i = 0; i < this.configurations.length; i++) {  
            this.backupDatabase(this.configurations[i]);
        }
    }

    transformRecord(record: any) {
        var state = this.applicationsStates[this.stepProgress - 2];
        this.$http.post('Database/TransformRecord', {
            id: record.item1,
        }).then((response: any) => {
            state.index++;
            record.item3 = response.data;

            //If finished, go to next application
            if (this.transformationProgressPercentage == 100)
                this.stepProgress++;

        }).catch((error: any) => {
        });
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
