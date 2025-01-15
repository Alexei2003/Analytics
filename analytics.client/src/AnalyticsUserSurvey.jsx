import { useEffect, useState } from 'react';
//import './AnalyticsUserSurvey.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import 'chart.js/auto';
import { AddButtonAnalyticsUserSurvey, AddButtonUserProfile, AddButtonAdminPanel } from './funcs/Buttons';

function AnalyticsUserSurvey() {

    const { surveyId, employeeId } = useParams();

    const [result, setResult] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getUserResult();
    }, []);

    function AddLine(name, elem) {
        if (!elem) {
            return null;
        }

        return (
            <tr className="elemListSurveys">
                <td className="text-left">
                    <label>{name}</label>
                </td>
                <td className="text-right">
                    <label> {elem} </label>
                </td>
            </tr>
        )
    }

    return (
        <>
            <div className="container">
                <div className="profile-buttons">
                    {AddButtonAnalyticsUserSurvey(navigate, employeeId)}
                    {AddButtonUserProfile(navigate)}
                </div>
                {result && (
                    <>
                        <div>
                            <label>Набраный балл: {result.score || "0"} </label>
                        </div>
                        <div>
                            <table className="tableListSurveys">
                                <thead>
                                    <tr className="elemListSurveys">
                                        <th>
                                            <label>Вопрос</label>
                                        </th>
                                        <th>
                                            <label>Ответ</label>
                                        </th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {result.answers.map((answer) => (
                                        <>
                                            {AddLine(answer.question, answer.answer)}
                                        </>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </>
                )}
            </div>
        </>
    );

    async function getUserResult() {
        try {
            const response = await fetch(`analyticsusersurvey/user?surveyId=${surveyId}&employeeId=${employeeId}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setResult(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }

}

export default AnalyticsUserSurvey;
