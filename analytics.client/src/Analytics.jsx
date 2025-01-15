import { useEffect, useState } from 'react';
import './Analytics.css';
import { useNavigate } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import { Pie, Bar } from 'react-chartjs-2';
import 'chart.js/auto';
import { AddButtonListEmployees, AddButtonUserProfile, AddButtonHRPanel, AddButtonAdminPanel } from './funcs/Buttons';

function Analytics() {
    const [genderChart, setGenderChart] = useState(null);
    const [maritalStatusChart, setMaritalStatusChart] = useState(null);
    const [ageChart, setAgeChart] = useState(null);
    const [workExperienceChart, setWorkExperienceChart] = useState(null);
    const [workExperienceMeanChart, setWorkExperienceMeanChart] = useState(null);
    const [workExperienceMean, setWorkExperienceMean] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getAnalytics();
    }, []);

    return (
        <>
            <div className="container">
                <div className="profile-buttons">
                    {AddButtonListEmployees(navigate)}
                    {AddButtonHRPanel(navigate)}
                    {AddButtonAdminPanel(navigate)}
                    {AddButtonUserProfile(navigate)}
                </div>
            </div>
            <div className="chart-container">
                <h3>Круговая диаграмма по полу</h3>
                {genderChart ? (
                    <div className="chart">
                        <Pie
                            data={genderChart}
                            options={{
                                plugins: { legend: { labels: { color: 'white' } } },
                            }}
                        />
                    </div>
                ) : null}
            </div>
            <div className="chart-container">
                <h3>Круговая диаграмма по семейному положению</h3>
                {maritalStatusChart ? (
                    <div className="chart">
                        <Pie
                            data={maritalStatusChart}
                            options={{
                                plugins: { legend: { labels: { color: 'white' } } },
                            }}
                        />
                    </div>
                ) : null}
            </div>
            <div className="chart-container">
                <h3>Столбчатая диаграмма по возрасту</h3>
                {ageChart ? (
                    <div className="chart">
                        <Bar
                            data={ageChart}
                            options={{
                                plugins: { legend: { labels: { color: 'white' } } },
                                scales: {
                                    y: { ticks: { color: 'white' } },
                                    x: { ticks: { color: 'white' } }
                                }
                            }}
                        />
                    </div>
                ) : null}
            </div>
            <div className="chart-container">
                <h3>Столбчатая диаграмма по стажу</h3>
                {workExperienceChart ? (
                    <div className="chart">
                        <Bar
                            data={workExperienceChart}
                            options={{
                                plugins: { legend: { labels: { color: 'white' } } },
                                scales: {
                                    y: { ticks: { color: 'white' } },
                                    x: { ticks: { color: 'white' } }
                                }
                            }}
                        />
                    </div>
                ) : null}
            </div>
            <div className="chart-container">
                <h3>Процентное разделение по стажу относительно среднего</h3>
                <label>Средний стаж: {workExperienceMean || "0"} </label>
                {workExperienceMeanChart ? (
                    <div className="chart">
                        <Pie
                            data={workExperienceMeanChart}
                            options={{
                                plugins: { legend: { labels: { color: 'white' } } },
                            }}
                        />
                    </div>
                ) : null}
            </div>
        </>
    );

    async function getAnalytics() {
        const colors = [
            '#36A2EB', // Синий
            '#FF6384', // Розовый
            '#FFCE56', // Желтый
            '#4BC0C0', // Бирюзовый
            '#9966FF', // Фиолетовый
            '#FF9F40', // Оранжевый
            '#FF5733', // Красный
            '#C70039', // Темно-красный
            '#900C3F', // Бордовый
            '#581845', // Темно-бордовый
            '#DAF7A6', // Светло-зеленый
            '#FFC300', // Золотой
            '#FF5733', // Оранжево-красный
            '#C70039', // Ярко-красный
            '#900C3F', // Темно-бордовый
            '#581845', // Темно-фиолетовый
            '#FFC300', // Светло-желтый
            '#DAF7A6', // Светло-зеленый
            '#FF6F61', // Персиковый
            '#6B5B95', // Сине-фиолетовый
            '#88B04B'  // Зеленый
        ];

        try {
            const response = await fetch(`analytics`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setWorkExperienceMean(data.workExperienceMean);

                var chartData = {
                    labels: Object.keys(data.genderChart.data),
                    datasets: [
                        {
                            data: Object.values(data.genderChart.data),
                            backgroundColor: colors,
                        }
                    ]
                };
                setGenderChart(chartData)

                chartData = {
                    labels: Object.keys(data.maritalStatusChart.data),
                    datasets: [
                        {
                            data: Object.values(data.maritalStatusChart.data),
                            backgroundColor: colors,
                        }
                    ]
                };
                setMaritalStatusChart(chartData)

                chartData = {
                    labels: Object.keys(data.ageChart.data),
                    datasets: [
                        {
                            label: 'Возраст', 
                            data: Object.values(data.ageChart.data),
                            backgroundColor: colors,
                        }
                    ]
                };
                setAgeChart(chartData)

                chartData = {
                    labels: Object.keys(data.workExperienceChart.data),
                    datasets: [
                        {
                            label: 'Стаж', 
                            data: Object.values(data.workExperienceChart.data),
                            backgroundColor: colors,
                        }
                    ]
                };
                setWorkExperienceChart(chartData)

                chartData = {
                    labels: Object.keys(data.workExperienceMeanChart.data),
                    datasets: [
                        {
                            data: Object.values(data.workExperienceMeanChart.data),
                            backgroundColor: colors,
                        }
                    ]
                };
                setWorkExperienceMeanChart(chartData)

            }
            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            //navigate('/404');
        }
    }
}

export default Analytics;