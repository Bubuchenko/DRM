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

    stepProgress: number = 2;

    @Prop({ default: {} })
    params!: any;

    @Prop({ default: {} })
    configurations!: any;

    @Watch('value')
    onvalueChanged(val: any, oldVal: any)
    {
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
            for (var i = 0; i < this.params[this.stepProgress - 2].nonCompliantRecordSets.length; i++) {
                var record = this.params[this.stepProgress - 2].nonCompliantRecordSets[i];

                console.log(record);
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
