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

    @Prop({ default: {} })
    params!: any;

    @Prop({ default: {} })
    configurations!: any;

    @Watch('value')
    onvalueChanged(val: any, oldVal: any) {
        if (val == true) {
            this.BackupDatabases();
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
            //For each task in application
            for (var i = 0; i < this.params[this.stepProgress - 2].nonCompliantRecordSets.length; i++) {
                var task = this.params[this.stepProgress - 2].nonCompliantRecordSets[i];

                //For each record in the task
                for (var x = 0; x < task.records.length; x++) {
                    var record = task.records[x];

                    console.log(record);
                }
            }
        }
    }

    BackupDatabases() {
        for (var i = 0; i < this.configurations.length; i++) {
            this.backupDatabase(this.configurations[i]);
        }

        this.stepProgress++;
    }

    backupDatabase(configuration: any) {
        this.$http.post('Database/Backup', {
            id: configuration.id,
        }).then((response: any) => {
            console.log(response);
            configuration.backupComplete = response.data;
            }).catch((error: any) => {
        });
    }
}
